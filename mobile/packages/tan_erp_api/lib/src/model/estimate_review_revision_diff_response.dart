//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_revision_diff_response.g.dart';

/// EstimateReviewRevisionDiffResponse
///
/// Properties:
/// * [previousRevisionNo] 
/// * [previousGrandTotal] 
/// * [grandTotalDelta] 
/// * [addedWorkItems] 
/// * [removedWorkItems] 
/// * [changedWorkItems] 
@BuiltValue()
abstract class EstimateReviewRevisionDiffResponse implements Built<EstimateReviewRevisionDiffResponse, EstimateReviewRevisionDiffResponseBuilder> {
  @BuiltValueField(wireName: r'previousRevisionNo')
  int? get previousRevisionNo;

  @BuiltValueField(wireName: r'previousGrandTotal')
  double? get previousGrandTotal;

  @BuiltValueField(wireName: r'grandTotalDelta')
  double? get grandTotalDelta;

  @BuiltValueField(wireName: r'addedWorkItems')
  int? get addedWorkItems;

  @BuiltValueField(wireName: r'removedWorkItems')
  int? get removedWorkItems;

  @BuiltValueField(wireName: r'changedWorkItems')
  int? get changedWorkItems;

  EstimateReviewRevisionDiffResponse._();

  factory EstimateReviewRevisionDiffResponse([void updates(EstimateReviewRevisionDiffResponseBuilder b)]) = _$EstimateReviewRevisionDiffResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewRevisionDiffResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewRevisionDiffResponse> get serializer => _$EstimateReviewRevisionDiffResponseSerializer();
}

class _$EstimateReviewRevisionDiffResponseSerializer implements PrimitiveSerializer<EstimateReviewRevisionDiffResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewRevisionDiffResponse, _$EstimateReviewRevisionDiffResponse];

  @override
  final String wireName = r'EstimateReviewRevisionDiffResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewRevisionDiffResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.previousRevisionNo != null) {
      yield r'previousRevisionNo';
      yield serializers.serialize(
        object.previousRevisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.previousGrandTotal != null) {
      yield r'previousGrandTotal';
      yield serializers.serialize(
        object.previousGrandTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.grandTotalDelta != null) {
      yield r'grandTotalDelta';
      yield serializers.serialize(
        object.grandTotalDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.addedWorkItems != null) {
      yield r'addedWorkItems';
      yield serializers.serialize(
        object.addedWorkItems,
        specifiedType: const FullType(int),
      );
    }
    if (object.removedWorkItems != null) {
      yield r'removedWorkItems';
      yield serializers.serialize(
        object.removedWorkItems,
        specifiedType: const FullType(int),
      );
    }
    if (object.changedWorkItems != null) {
      yield r'changedWorkItems';
      yield serializers.serialize(
        object.changedWorkItems,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewRevisionDiffResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewRevisionDiffResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'previousRevisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.previousRevisionNo = valueDes;
          break;
        case r'previousGrandTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.previousGrandTotal = valueDes;
          break;
        case r'grandTotalDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grandTotalDelta = valueDes;
          break;
        case r'addedWorkItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.addedWorkItems = valueDes;
          break;
        case r'removedWorkItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.removedWorkItems = valueDes;
          break;
        case r'changedWorkItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.changedWorkItems = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewRevisionDiffResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewRevisionDiffResponseBuilder();
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


