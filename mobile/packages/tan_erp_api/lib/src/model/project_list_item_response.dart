//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_customer_response.dart';
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_list_item_response.g.dart';

/// ProjectListItemResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
/// * [status] 
/// * [plannedStartDate] 
/// * [owner] 
/// * [customer] 
/// * [contractAmount] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class ProjectListItemResponse implements Built<ProjectListItemResponse, ProjectListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'plannedStartDate')
  Date? get plannedStartDate;

  @BuiltValueField(wireName: r'owner')
  ProjectPersonResponse? get owner;

  @BuiltValueField(wireName: r'customer')
  ProjectCustomerResponse? get customer;

  @BuiltValueField(wireName: r'contractAmount')
  double? get contractAmount;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  ProjectListItemResponse._();

  factory ProjectListItemResponse([void updates(ProjectListItemResponseBuilder b)]) = _$ProjectListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectListItemResponse> get serializer => _$ProjectListItemResponseSerializer();
}

class _$ProjectListItemResponseSerializer implements PrimitiveSerializer<ProjectListItemResponse> {
  @override
  final Iterable<Type> types = const [ProjectListItemResponse, _$ProjectListItemResponse];

  @override
  final String wireName = r'ProjectListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.plannedStartDate != null) {
      yield r'plannedStartDate';
      yield serializers.serialize(
        object.plannedStartDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.owner != null) {
      yield r'owner';
      yield serializers.serialize(
        object.owner,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.customer != null) {
      yield r'customer';
      yield serializers.serialize(
        object.customer,
        specifiedType: const FullType(ProjectCustomerResponse),
      );
    }
    if (object.contractAmount != null) {
      yield r'contractAmount';
      yield serializers.serialize(
        object.contractAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectListItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'plannedStartDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedStartDate = valueDes;
          break;
        case r'owner':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.owner.replace(valueDes);
          break;
        case r'customer':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectCustomerResponse),
          ) as ProjectCustomerResponse?;
          if (valueDes == null) continue;
          result.customer.replace(valueDes);
          break;
        case r'contractAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.contractAmount = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectListItemResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


