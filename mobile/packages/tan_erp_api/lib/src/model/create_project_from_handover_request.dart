//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_project_from_handover_request.g.dart';

/// CreateProjectFromHandoverRequest
///
/// Properties:
/// * [quotationId] 
/// * [expectedQuotationVersion] 
/// * [ownerUserId] 
/// * [plannedStartDate] 
/// * [name] 
@BuiltValue()
abstract class CreateProjectFromHandoverRequest implements Built<CreateProjectFromHandoverRequest, CreateProjectFromHandoverRequestBuilder> {
  @BuiltValueField(wireName: r'quotationId')
  String? get quotationId;

  @BuiltValueField(wireName: r'expectedQuotationVersion')
  String? get expectedQuotationVersion;

  @BuiltValueField(wireName: r'ownerUserId')
  String? get ownerUserId;

  @BuiltValueField(wireName: r'plannedStartDate')
  Date? get plannedStartDate;

  @BuiltValueField(wireName: r'name')
  String? get name;

  CreateProjectFromHandoverRequest._();

  factory CreateProjectFromHandoverRequest([void updates(CreateProjectFromHandoverRequestBuilder b)]) = _$CreateProjectFromHandoverRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateProjectFromHandoverRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateProjectFromHandoverRequest> get serializer => _$CreateProjectFromHandoverRequestSerializer();
}

class _$CreateProjectFromHandoverRequestSerializer implements PrimitiveSerializer<CreateProjectFromHandoverRequest> {
  @override
  final Iterable<Type> types = const [CreateProjectFromHandoverRequest, _$CreateProjectFromHandoverRequest];

  @override
  final String wireName = r'CreateProjectFromHandoverRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateProjectFromHandoverRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.quotationId != null) {
      yield r'quotationId';
      yield serializers.serialize(
        object.quotationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.expectedQuotationVersion != null) {
      yield r'expectedQuotationVersion';
      yield serializers.serialize(
        object.expectedQuotationVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.ownerUserId != null) {
      yield r'ownerUserId';
      yield serializers.serialize(
        object.ownerUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.plannedStartDate != null) {
      yield r'plannedStartDate';
      yield serializers.serialize(
        object.plannedStartDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateProjectFromHandoverRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateProjectFromHandoverRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'quotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationId = valueDes;
          break;
        case r'expectedQuotationVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedQuotationVersion = valueDes;
          break;
        case r'ownerUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.ownerUserId = valueDes;
          break;
        case r'plannedStartDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedStartDate = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateProjectFromHandoverRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateProjectFromHandoverRequestBuilder();
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


