//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_rate_override_request.g.dart';

/// BranchRateOverrideRequest
///
/// Properties:
/// * [referenceRate] 
/// * [minimumCharge] 
/// * [effectiveFrom] 
/// * [effectiveTo] 
/// * [reasonCode] 
/// * [reasonDetail] 
/// * [sourceReference] 
@BuiltValue()
abstract class BranchRateOverrideRequest implements Built<BranchRateOverrideRequest, BranchRateOverrideRequestBuilder> {
  @BuiltValueField(wireName: r'referenceRate')
  double? get referenceRate;

  @BuiltValueField(wireName: r'minimumCharge')
  double? get minimumCharge;

  @BuiltValueField(wireName: r'effectiveFrom')
  Date? get effectiveFrom;

  @BuiltValueField(wireName: r'effectiveTo')
  Date? get effectiveTo;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  @BuiltValueField(wireName: r'reasonDetail')
  String? get reasonDetail;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  BranchRateOverrideRequest._();

  factory BranchRateOverrideRequest([void updates(BranchRateOverrideRequestBuilder b)]) = _$BranchRateOverrideRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchRateOverrideRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchRateOverrideRequest> get serializer => _$BranchRateOverrideRequestSerializer();
}

class _$BranchRateOverrideRequestSerializer implements PrimitiveSerializer<BranchRateOverrideRequest> {
  @override
  final Iterable<Type> types = const [BranchRateOverrideRequest, _$BranchRateOverrideRequest];

  @override
  final String wireName = r'BranchRateOverrideRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchRateOverrideRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.referenceRate != null) {
      yield r'referenceRate';
      yield serializers.serialize(
        object.referenceRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.minimumCharge != null) {
      yield r'minimumCharge';
      yield serializers.serialize(
        object.minimumCharge,
        specifiedType: const FullType(double),
      );
    }
    if (object.effectiveFrom != null) {
      yield r'effectiveFrom';
      yield serializers.serialize(
        object.effectiveFrom,
        specifiedType: const FullType(Date),
      );
    }
    if (object.effectiveTo != null) {
      yield r'effectiveTo';
      yield serializers.serialize(
        object.effectiveTo,
        specifiedType: const FullType(Date),
      );
    }
    if (object.reasonCode != null) {
      yield r'reasonCode';
      yield serializers.serialize(
        object.reasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reasonDetail != null) {
      yield r'reasonDetail';
      yield serializers.serialize(
        object.reasonDetail,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceReference != null) {
      yield r'sourceReference';
      yield serializers.serialize(
        object.sourceReference,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BranchRateOverrideRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchRateOverrideRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'referenceRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.referenceRate = valueDes;
          break;
        case r'minimumCharge':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minimumCharge = valueDes;
          break;
        case r'effectiveFrom':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveFrom = valueDes;
          break;
        case r'effectiveTo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.effectiveTo = valueDes;
          break;
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonCode = valueDes;
          break;
        case r'reasonDetail':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonDetail = valueDes;
          break;
        case r'sourceReference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceReference = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BranchRateOverrideRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchRateOverrideRequestBuilder();
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


