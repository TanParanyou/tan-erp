//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/service_project_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'warranty_response.g.dart';

/// WarrantyResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [project] 
/// * [installationJobId] 
/// * [installationNumber] 
/// * [startDate] 
/// * [endDate] 
/// * [months] 
/// * [isActive] 
/// * [serviceRequestCount] 
@BuiltValue()
abstract class WarrantyResponse implements Built<WarrantyResponse, WarrantyResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'project')
  ServiceProjectRefResponse? get project;

  @BuiltValueField(wireName: r'installationJobId')
  String? get installationJobId;

  @BuiltValueField(wireName: r'installationNumber')
  String? get installationNumber;

  @BuiltValueField(wireName: r'startDate')
  Date? get startDate;

  @BuiltValueField(wireName: r'endDate')
  Date? get endDate;

  @BuiltValueField(wireName: r'months')
  int? get months;

  @BuiltValueField(wireName: r'isActive')
  bool? get isActive;

  @BuiltValueField(wireName: r'serviceRequestCount')
  int? get serviceRequestCount;

  WarrantyResponse._();

  factory WarrantyResponse([void updates(WarrantyResponseBuilder b)]) = _$WarrantyResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WarrantyResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WarrantyResponse> get serializer => _$WarrantyResponseSerializer();
}

class _$WarrantyResponseSerializer implements PrimitiveSerializer<WarrantyResponse> {
  @override
  final Iterable<Type> types = const [WarrantyResponse, _$WarrantyResponse];

  @override
  final String wireName = r'WarrantyResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WarrantyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(ServiceProjectRefResponse),
      );
    }
    if (object.installationJobId != null) {
      yield r'installationJobId';
      yield serializers.serialize(
        object.installationJobId,
        specifiedType: const FullType(String),
      );
    }
    if (object.installationNumber != null) {
      yield r'installationNumber';
      yield serializers.serialize(
        object.installationNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.startDate != null) {
      yield r'startDate';
      yield serializers.serialize(
        object.startDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.endDate != null) {
      yield r'endDate';
      yield serializers.serialize(
        object.endDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.months != null) {
      yield r'months';
      yield serializers.serialize(
        object.months,
        specifiedType: const FullType(int),
      );
    }
    if (object.isActive != null) {
      yield r'isActive';
      yield serializers.serialize(
        object.isActive,
        specifiedType: const FullType(bool),
      );
    }
    if (object.serviceRequestCount != null) {
      yield r'serviceRequestCount';
      yield serializers.serialize(
        object.serviceRequestCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WarrantyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WarrantyResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServiceProjectRefResponse),
          ) as ServiceProjectRefResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'installationJobId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.installationJobId = valueDes;
          break;
        case r'installationNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.installationNumber = valueDes;
          break;
        case r'startDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.startDate = valueDes;
          break;
        case r'endDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.endDate = valueDes;
          break;
        case r'months':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.months = valueDes;
          break;
        case r'isActive':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isActive = valueDes;
          break;
        case r'serviceRequestCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.serviceRequestCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WarrantyResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WarrantyResponseBuilder();
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


